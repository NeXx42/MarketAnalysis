
"use client"

import * as api from "@shared/api.client";

import { AssetType } from '@/types/assetType';
import { AreaSeries, createChart, ColorType } from 'lightweight-charts';
import { useEffect, useRef, useState } from 'react';
import { graphData } from "@/types/graphData";

interface Props {
    assetType: AssetType
}

export default function (props: Props) {

    const [loading, setLoading] = useState(false);
    const [graph, setGraph] = useState<graphData | undefined>(undefined);

    useEffect(() => {
        setLoading(true);
        api.fetchAssetGraph(props.assetType)
            .then(setGraph)
            .finally(() => setLoading(false));

    }, [props.assetType])



    const chartContainerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!chartContainerRef.current || graph === undefined)
            return;
        const graphData: any = {
            data: graph?.datapoints.map(x => ({
                time: x.date.toISOString().split('T')[0],
                value: x.close
            })),
            colors: {
                backgroundColor: 'black',
                lineColor: '#2962FF',
                textColor: 'white',
                areaTopColor: '#2962FF',
                areaBottomColor: 'rgba(41, 98, 255, 0.28)',
            },
        };
        console.log(graphData);

        const handleResize = () => {
            chart.applyOptions({ width: chartContainerRef.current!.clientWidth });
        };

        const chart = createChart(chartContainerRef.current, {
            layout: {
                background: { type: ColorType.Solid, color: graphData.colors.backgroundColor },
                textColor: graphData.colors.textColor,
            },
            width: chartContainerRef.current.clientWidth,
            height: 300,
        });
        chart.timeScale().fitContent();

        const newSeries = chart.addSeries(AreaSeries, graphData.colors);
        newSeries.setData(graphData.data);

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