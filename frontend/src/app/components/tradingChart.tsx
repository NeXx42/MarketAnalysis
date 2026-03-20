
"use client"

import * as api from "@shared/api.client";

import { AssetType } from '@/types/assetType';
import { AreaSeries, createChart, ColorType, CandlestickSeries, LineSeries, createSeriesMarkers } from 'lightweight-charts';
import { useEffect, useRef, useState } from 'react';
import { graphData } from "@/types/graphData";
import { AnalysisTrend } from "@/types/analysisTrends";

interface Props {
    assetType: AssetType
}

export default function (props: Props) {

    const [loading, setLoading] = useState(false);
    const [graph, setGraph] = useState<graphData | undefined>(undefined);

    useEffect(() => {
        setLoading(true);
        api.fetchAssetGraph(props.assetType, [AnalysisTrend.AMA])
            .then(setGraph)
            .finally(() => setLoading(false));

    }, [props.assetType])



    const chartContainerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!chartContainerRef.current || graph === undefined)
            return;

        const formattedCandleData = graph?.datapoints.map(x => ({
            time: x.date.toISOString().split('T')[0],
            open: x.open,
            close: x.close,
            high: x.high,
            low: x.low
        }))

        const handleResize = () => {
            chart.applyOptions({ width: chartContainerRef.current!.clientWidth });
        };

        const chart = createChart(chartContainerRef.current, {
            layout: {
                background: { type: ColorType.Solid, color: 'black' },
                textColor: 'white',
            },
            width: chartContainerRef.current.clientWidth,
            height: 300,
        });
        chart.timeScale().fitContent();

        const candlestickSeries = chart.addSeries(CandlestickSeries, {
            upColor: '#26a69a', downColor: '#ef5350', borderVisible: false,
            wickUpColor: '#26a69a', wickDownColor: '#ef5350',
        });
        candlestickSeries.setData(formattedCandleData);

        if (AnalysisTrend[AnalysisTrend.AMA] in graph.trends) {
            const markers: any[] = [];
            const emaData = graph.trends[AnalysisTrend[AnalysisTrend.AMA]];

            for (let i = 1; i < emaData.length; i++) {
                const prevShort = emaData[i - 1].ema10;
                const prevLong = emaData[i - 1].ema50;

                const currShort = emaData[i].ema10;
                const currLong = emaData[i].ema50;

                const time = formattedCandleData[i].time;

                if (prevShort <= prevLong && currShort > currLong) {
                    markers.push({
                        time,
                        position: 'belowBar',
                        color: 'green',
                        shape: 'arrowUp',
                        text: 'Uptrend'
                    });
                }

                if (prevShort >= prevLong && currShort < currLong) {
                    markers.push({
                        time,
                        position: 'aboveBar',
                        color: 'red',
                        shape: 'arrowDown',
                        text: 'Downtrend'
                    });
                }
            }

            createSeriesMarkers(candlestickSeries, markers);
        }

        chart.timeScale().fitContent();
        window.addEventListener('resize', handleResize);

        return () => {
            window.removeEventListener('resize', handleResize);
            chart.remove();
        };
    },
        [graph]
    );

    return (
        <div>
            {!loading && (
                <div ref={chartContainerRef} />
            )}
        </div>
    );
};