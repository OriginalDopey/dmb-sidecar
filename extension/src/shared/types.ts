export interface PageSlot {
  order: number;
  position?: string;
  playerName?: string;
  bats?: string;
  salary?: string;
  section?: string;
}

export interface PageContext {
  pageType: string;
  url: string;
  lineupName?: string;
  curTeam?: string;
  slots?: PageSlot[];
  extra?: Record<string, string>;
}

export interface AdviseRequest {
  question: string;
  context: PageContext;
}

export interface Citation {
  source: string;
  label: string;
  snippet?: string;
}

export interface AdviseResponse {
  answer: string;
  citations: Citation[];
  elapsedMs: number;
  warning?: string;
  /** Routed handler kind from Lineup Lab explain (e.g. DhAssignment). */
  questionKind?: string;
}

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

export interface LineupSwap {
  position: string;
  from: string;
  to: string;
  gain: number;
}

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
  engine?: string;
}

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
