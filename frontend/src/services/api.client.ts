"use client"

import { post, get } from "./api.shared";

import { graphData } from "@/types/graphData";
import { AssetType } from "@/types/assetType";
import { AnalysisTrend } from "@/types/analysisTrends";

export async function fetchAssetGraph(assetType: AssetType, trends: AnalysisTrend[]): Promise<graphData> {
    const res = await post<graphData>(`graph/${assetType}`, {
        trends
    });

    return {
        ...res!,
        datapoints: res!.datapoints.map(x => ({
            ...x,
            date: new Date(x.date)
        })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    }
}