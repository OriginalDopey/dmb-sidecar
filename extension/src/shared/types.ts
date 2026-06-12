/**
 * @file Shared type contracts — page context, API payloads, and extension messages.
 *
 * **Purpose:** Single source of truth for data crossing content script, service
 * worker, side panel, and sidecar HTTP API boundaries.
 *
 * **Message flow:** `SidecarMessage` discriminated union routes all extension IPC;
 * `PageContext` / `AdviseRequest` / `LineupAnalyzeResponse` serialize to API JSON.
 *
 * **Dependencies:** None (pure TypeScript types).
 */

/** One roster or lineup slot scraped from the DOM. */
export interface PageSlot {
  /** Batting order (1–9 on lineup) or row sequence on roster tables. */
  order: number;
  /** Defensive position abbreviation when known. */
  position?: string;
  /** `"Last, First"` player label. */
  playerName?: string;
  bats?: string;
  /** Formatted salary string from roster table. */
  salary?: string;
  /** Roster section: `batter`, `pitcher`, `ir`, etc. */
  section?: string;
}

/** Structured snapshot of the active ImagineSports screen. */
export interface PageContext {
  /** Adapter key: `lineup`, `roster`, or `unknown`. */
  pageType: string;
  /** Full page URL at scrape time. */
  url: string;
  lineupName?: string;
  /** Team ID query param when present. */
  curTeam?: string;
  slots?: PageSlot[];
  /** Adapter-specific string metadata (JSON blobs, finance lines, etc.). */
  extra?: Record<string, string>;
}

/** Request body for `POST /advise`. */
export interface AdviseRequest {
  question: string;
  context: PageContext;
}

/** Source attribution returned with generic advise answers. */
export interface Citation {
  source: string;
  label: string;
  snippet?: string;
}

/** Response payload from `/advise` and `/lineup/explain`. */
export interface AdviseResponse {
  answer: string;
  citations: Citation[];
  elapsedMs: number;
  warning?: string;
  /** Routed handler kind from Lineup Lab explain (e.g. `DhAssignment`). */
  questionKind?: string;
}

/** One batting-order row in lineup analyze output. */
export interface LineupSlotResult {
  order: number;
  position: string;
  player: string;
  rc600: number;
  def: number;
  total: number;
  salary: number;
  inPool: boolean;
  ops?: number;
  obp?: number;
  hrf?: number;
  batPlat?: number;
  run?: string;
  injury?: string;
  rangeGrade?: string;
  err?: number | null;
}

/** Single position swap between current and recommended lineups. */
export interface LineupSwap {
  position: string;
  from: string;
  to: string;
  gain: number;
}

/** Full response from `POST /lineup/analyze`. */
export interface LineupAnalyzeResponse {
  lineupName: string;
  pitcherSide: string;
  currentLineup: LineupSlotResult[];
  recommendedLineup: LineupSlotResult[];
  currentTotal: number;
  recommendedTotal: number;
  delta: number;
  swaps: LineupSwap[];
  notes: string[];
  platoonHints: string[];
  summary: string;
  chart: { labels: string[]; current: number[]; recommended: number[] };
  poolSize: number;
  /** Optimizer backend identifier (`dmb-config`, `rc-def-fallback`, etc.). */
  engine?: string;
}

/**
 * Discriminated union of all extension runtime messages.
 *
 * Handlers return `true` from `onMessage` when `sendResponse` is async.
 */
export type SidecarMessage =
  | { type: "PAGE_CONTEXT"; context: PageContext }
  | { type: "GET_PAGE_CONTEXT" }
  | { type: "REFRESH_CONTEXT" }
  | { type: "ADVISE"; question: string }
  | { type: "LINEUP_ANALYZE" }
  | { type: "LINEUP_EXPLAIN"; question: string; lineup?: LineupAnalyzeResponse }
  | { type: "ADVISE_RESULT"; response: AdviseResponse }
  | { type: "LINEUP_RESULT"; response: LineupAnalyzeResponse; context?: PageContext }
  | { type: "ADVISE_ERROR"; error: string }
  | { type: "CONTEXT_UPDATE"; context: PageContext };
