import Image from "next/image";
import styles from "./page.module.css";
import TradingChart from "./components/tradingChart";
import { AssetType } from "@/types/assetType";

export default function Home() {
  return (
    <div>
      <TradingChart />
    </div>
  );
}
