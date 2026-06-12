export interface PageSlot {
  order: number;
  position?: string;
  playerName?: string;
  bats?: string;
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
}

export type SidecarMessage =
  | { type: "PAGE_CONTEXT"; context: PageContext }
  | { type: "GET_PAGE_CONTEXT" }
  | { type: "ADVISE"; question: string }
  | { type: "ADVISE_RESULT"; response: AdviseResponse }
  | { type: "ADVISE_ERROR"; error: string }
  | { type: "CONTEXT_UPDATE"; context: PageContext };
