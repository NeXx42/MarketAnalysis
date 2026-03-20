import { AnalysisTrend } from "./analysisTrends";
import { AssetType } from "./assetType";

export interface graphData {
    assetType: AssetType,
    datapoints: graphDataPoint[],
    trends: Record<string, any>
}

export interface graphDataPoint {
    date: Date,
    open: number,
    close: number,
    high: number,
    low: number,
}