"use client"

import { post, get } from "./api.shared";

import { graphData } from "@/types/graphData";
import { AssetType } from "@/types/assetType";

export async function fetchAssetGraph(assetType: AssetType): Promise<graphData> {
    const res = await get<graphData>(`graph/${assetType}`);
    return {
        ...res!,
        datapoints: res!.datapoints.map(x => ({
            ...x,
            date: new Date(x.date)
        })).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
    }
}