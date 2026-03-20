import { AssetType } from "./assetType";

export interface graphData {
    assetType: AssetType,
    datapoints: graphDataPoint[]
}

export interface graphDataPoint {
    date: Date,
    open: number,
    close: number,
    high: number,
    low: number,
}