import { fmtAvg, fmtDef, fmtRc600 } from "../shared/lineup-format.js";
import type { LineupAnalyzeResponse, PageContext, SidecarMessage } from "../shared/types.js";

const pageContextCard = document.getElementById("page-context-card")!;
const pageLabel = document.getElementById("page-label")!;
const contextPreview = document.getElementById("context-preview")!;
const answerEl = document.getElementById("answer")!;
const chartArea = document.getElementById("chart-area")!;
const citationsEl = document.getElementById("citations")!;
const warningEl = document.getElementById("warning")!;
const statusEl = document.getElementById("status")!;
const questionInput = document.getElementById("question") as HTMLTextAreaElement;
const askBtn = document.getElementById("ask-btn")!;
const askHeading = document.getElementById("ask-heading")!;
const explainBtn = document.getElementById("explain-btn")!;
const lineupLab = document.getElementById("lineup-lab")!;
const lineupResults = document.getElementById("lineup-results")!;
const lineupTitle = document.getElementById("lineup-title")!;
const optimizeBtn = document.getElementById("optimize-btn")!;
const lineupPromptsEl = document.getElementById("lineup-prompts")!;

/** Demo prompts — each maps to a routed handler in LineupExplainRouter. */
const LINEUP_PROMPTS: { label: string; question: string }[] = [
  { label: "Main diff", question: "Explain the main differences between my lineup and the recommendation." },
  { label: "DH", question: "Why not Cobb or Ruth at DH?" },
  { label: "Bat #4", question: "Why bat Ruth at #4?" },
  { label: "At SS", question: "Why Mackanin at SS?" },
  { label: "Knight > Mack", question: "Why Knight over Mackanin at SS?" },
];

let currentContext: PageContext | null = null;
let lastLineupResult: LineupAnalyzeResponse | null = null;

function setStatus(text: string, isError = false): void {
  statusEl.textContent = text;
  statusEl.className = isError ? "status error" : "status";
}

function escapeHtml(s: string): string {
  return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

function isLineupPage(ctx: PageContext | null = currentContext): boolean {
  if (!ctx) return false;
  if (ctx.pageType === "lineup") return true;
  try {
    return new URL(ctx.url).pathname.includes("/manage/edit_lineup");
  } catch {
    return false;
  }
}

function renderLineupPrompts(): void {
  lineupPromptsEl.innerHTML = LINEUP_PROMPTS.map(
    (p, i) =>
      `<button type="button" class="chip" data-prompt-idx="${i}">${escapeHtml(p.label)}</button>`
  ).join("");
  lineupPromptsEl.querySelectorAll<HTMLButtonElement>(".chip").forEach((btn) => {
    btn.addEventListener("click", () => {
      const idx = Number(btn.dataset.promptIdx);
      const q = LINEUP_PROMPTS[idx]?.question ?? "";
      questionInput.value = q;
      ask(q);
    });
  });
}

function setLineupMode(active: boolean, ctx?: PageContext): void {
  pageContextCard.classList.toggle("hidden", active);
  lineupLab.classList.toggle("hidden", !active);
  lineupResults.classList.toggle("hidden", !active);
  explainBtn.classList.toggle("hidden", active);
  lineupPromptsEl.classList.toggle("hidden", !active);

  if (active && ctx) {
    const side = ctx.extra?.pitcherSide === "lhp" ? "vs LHP" : "vs RHP";
    lineupTitle.textContent = ctx.lineupName ? `${ctx.lineupName} · ${side}` : side;
    askHeading.textContent = "Explain";
    askBtn.textContent = "Explain recommendation";
    questionInput.placeholder = "e.g. why not Cobb at DH?";
  } else {
    askHeading.textContent = "Ask";
    askBtn.textContent = "Ask";
    questionInput.placeholder = "Ask about this screen…";
  }
}

function renderContext(ctx: PageContext): void {
  currentContext = ctx;
  const lineup = isLineupPage(ctx);
  setLineupMode(lineup, ctx);

  if (lineup) return;

  pageLabel.textContent = `${ctx.pageType} — ${new URL(ctx.url).pathname}`;
  const lines: string[] = [];

  if (ctx.extra?.teamName) {
    lines.push(`${ctx.extra.teamName}${ctx.extra.leagueName ? ` (${ctx.extra.leagueName} League)` : ""}`);
  }
  if (ctx.lineupName) lines.push(`Lineup: ${ctx.lineupName}`);
  if (ctx.curTeam) lines.push(`curTeam: ${ctx.curTeam}`);

  if (ctx.extra?.totalValue || ctx.extra?.cashBalance) {
    lines.push(
      `Cap: ${ctx.extra.totalValue ?? "?"} · Cash: ${ctx.extra.cashBalance ?? "?"}${ctx.extra.stadium ? ` · ${ctx.extra.stadium}` : ""}`
    );
  }

  if (ctx.pageType === "roster" && ctx.slots?.length) {
    const show = (label: string, section: string) => {
      const group = ctx.slots!.filter((s) => s.section === section);
      if (!group.length) return;
      lines.push(`${label} (${group.length}):`);
      group.slice(0, 6).forEach((s) => {
        lines.push(`  ${s.position ?? "?"} ${s.playerName ?? "?"} ${s.salary ?? ""}`.trim());
      });
      if (group.length > 6) lines.push(`  … +${group.length - 6} more`);
    };
    show("Position players", "batter");
    show("Pitchers", "pitcher");
    show("IR", "ir");
  }

  contextPreview.textContent = lines.length ? lines.join("\n") : "(no structured data)";
}

function formatSal(salary: number): string {
  if (salary >= 1_000_000) return `$${(salary / 1_000_000).toFixed(1)}M`;
  if (salary > 0) return `$${(salary / 1000).toFixed(0)}K`;
  return "—";
}

function lastName(player: string): string {
  return player.split(",")[0]?.trim() ?? player;
}

function shortInj(injury?: string): string {
  if (!injury) return "—";
  const m: Record<string, string> = {
    Normal: "Norm",
    Fragile: "Frag",
    Prone: "Prone",
    Iron: "Iron",
  };
  return m[injury] ?? injury.slice(0, 4);
}

function defClass(n: number | undefined): string {
  if (n != null && n < -0.05) return "neg";
  return "";
}

function fmtPlat(batPlat?: number): string {
  if (batPlat == null || batPlat === 0) return "—";
  return batPlat > 0 ? `+${batPlat}` : String(batPlat);
}

function platClass(batPlat?: number): string {
  if (batPlat == null || batPlat === 0) return "";
  return batPlat > 0 ? "plat-plus" : "plat-minus";
}

function fmtFld(slot: LineupAnalyzeResponse["currentLineup"][0]): string {
  if (slot.position === "DH") return "—";
  if (!slot.rangeGrade) return "—";
  return slot.err != null ? `${slot.rangeGrade}/${slot.err}` : slot.rangeGrade;
}

function tdNum(val: string, extra = ""): string {
  return `<td class="num ${extra}">${escapeHtml(val)}</td>`;
}

function tdText(val: string, cls = "", title = ""): string {
  const t = title ? ` title="${escapeHtml(title)}"` : "";
  return `<td class="${cls}"${t}>${escapeHtml(val)}</td>`;
}

function renderSideCells(
  slot: LineupAnalyzeResponse["currentLineup"][0],
  opts: { diffPos?: boolean; diffPlayer?: boolean }
): string {
  return `
    ${tdText(slot.position, "pos" + (opts.diffPos ? " diff" : ""))}
    ${tdText(lastName(slot.player), "player" + (opts.diffPlayer ? " diff" : ""), slot.player)}
    ${tdNum(fmtAvg(slot.obp, 3))}
    ${tdNum(fmtAvg(slot.ops, 3))}
    ${tdNum(fmtRc600(slot.rc600))}
    ${tdNum(fmtAvg(slot.hrf, 1))}
    ${tdText(slot.run || "—", "ctr")}
    ${tdNum(fmtPlat(slot.batPlat), platClass(slot.batPlat))}
    ${tdText(fmtFld(slot), "ctr fld")}
    ${tdNum(fmtDef(slot.def), defClass(slot.def))}
    ${tdText(shortInj(slot.injury), "ctr inj")}
    ${tdNum(formatSal(slot.salary), "sal")}`;
}

function renderLineupGrid(
  current: LineupAnalyzeResponse["currentLineup"],
  recommended: LineupAnalyzeResponse["currentLineup"],
  sideLabel: string
): string {
  const curByOrder = new Map(current.map((s) => [s.order, s]));
  const recByOrder = new Map(recommended.map((s) => [s.order, s]));
  const orders = [...new Set([...curByOrder.keys(), ...recByOrder.keys()])].sort((a, b) => a - b);

  const body = orders
    .map((o) => {
      const cur = curByOrder.get(o);
      const rec = recByOrder.get(o);
      if (!cur) return "";
      const recCells = rec
        ? renderSideCells(rec, {
            diffPos: rec.position !== cur.position,
            diffPlayer: rec.player !== cur.player,
          })
        : `<td colspan="12" class="empty">—</td>`;

      return `<tr>
        ${tdNum(String(o), "ord")}
        ${renderSideCells(cur, {})}
        <td class="sep"></td>
        ${recCells}
      </tr>`;
    })
    .join("");

  return `
    <p class="lineup-side-note">${escapeHtml(sideLabel)} · stats vs this pitching side</p>
    <div class="lineup-table-wrap">
      <table class="lineup-table">
        <colgroup>
          <col class="c-ord" />
          <col class="c-pos" /><col class="c-player" />
          <col class="c-obp" /><col class="c-ops" /><col class="c-rc" />
          <col class="c-hrf" /><col class="c-run" /><col class="c-plat" />
          <col class="c-fld" /><col class="c-def" /><col class="c-inj" /><col class="c-sal" />
          <col class="c-sep" />
          <col class="c-pos" /><col class="c-player" />
          <col class="c-obp" /><col class="c-ops" /><col class="c-rc" />
          <col class="c-hrf" /><col class="c-run" /><col class="c-plat" />
          <col class="c-fld" /><col class="c-def" /><col class="c-inj" /><col class="c-sal" />
        </colgroup>
        <thead>
          <tr class="group-row">
            <th colspan="13" class="group-hdr cur-hdr">Your lineup</th>
            <th class="sep"></th>
            <th colspan="12" class="group-hdr rec-hdr">Recommended</th>
          </tr>
          <tr class="col-row">
            <th>#</th>
            <th>Pos</th><th>Player</th>
            <th>OBP</th><th>OPS</th><th>RC/600</th><th>HRF</th><th>Run</th><th>Plat</th><th>Fld</th><th>Def</th><th>Inj</th><th>$</th>
            <th class="sep"></th>
            <th>Pos</th><th>Player</th>
            <th>OBP</th><th>OPS</th><th>RC/600</th><th>HRF</th><th>Run</th><th>Plat</th><th>Fld</th><th>Def</th><th>Inj</th><th>$</th>
          </tr>
        </thead>
        <tbody>${body}</tbody>
      </table>
    </div>`;
}

function mergePageSlots(
  pageSlots: PageContext["slots"],
  apiSlots: LineupAnalyzeResponse["currentLineup"]
): LineupAnalyzeResponse["currentLineup"] {
  if (!pageSlots?.length) return apiSlots;
  const apiByOrder = new Map(apiSlots.map((s) => [s.order, s]));
  return pageSlots.map((s) => {
    const api = apiByOrder.get(s.order);
    return {
      order: s.order,
      position: s.position ?? api?.position ?? "?",
      player: s.playerName ?? api?.player ?? "?",
      rc600: api?.rc600 ?? 0,
      def: api?.def ?? 0,
      total: api?.total ?? 0,
      salary: api?.salary ?? 0,
      inPool: api?.inPool ?? false,
      ops: api?.ops,
      obp: api?.obp,
      hrf: api?.hrf,
      batPlat: api?.batPlat,
      run: api?.run,
      injury: api?.injury,
      rangeGrade: api?.rangeGrade,
      err: api?.err,
    };
  });
}

function renderLineupResult(r: LineupAnalyzeResponse, pageSlots?: PageContext["slots"]): void {
  citationsEl.innerHTML = "";
  warningEl.textContent = "";

  const deltaSign = r.delta >= 0 ? "+" : "";
  const yourLineup = mergePageSlots(pageSlots, r.currentLineup);

  chartArea.innerHTML = `
    <div class="metric-row">
      <div class="metric"><div class="val">${r.currentTotal.toFixed(1)}</div><div class="lbl">Current</div></div>
      <div class="metric"><div class="val">${r.recommendedTotal.toFixed(1)}</div><div class="lbl">Recommended</div></div>
      <div class="metric"><div class="val">${deltaSign}${r.delta.toFixed(1)}</div><div class="lbl">Δ RC+def</div></div>
    </div>
    ${renderLineupGrid(yourLineup, r.recommendedLineup, r.pitcherSide)}
  `;

  answerEl.innerHTML = "";
  const engine =
    r.engine === "dmb-config"
      ? "implementation-plan rules"
      : r.engine === "rc-def-fallback"
        ? "RC+def fallback"
        : "lineup model";
  setStatus(`Lineup Lab · ${engine} · ${r.poolSize} in pool`);
}

chrome.runtime.onMessage.addListener((msg: SidecarMessage) => {
  if (msg.type === "CONTEXT_UPDATE") renderContext(msg.context);
});

function formatExplainAnswer(text: string): string {
  const paras = text.split(/\n\n+/).map((p) => {
    const lines = p
      .split("\n")
      .map((line) =>
        escapeHtml(line)
          .replace(/\*\*(.+?)\*\*/g, "<strong>$1</strong>")
      )
      .join("<br>");
    return `<p class="explain-p">${lines}</p>`;
  });
  return paras.join("");
}

function renderExplainResult(r: Extract<SidecarMessage, { type: "ADVISE_RESULT" }>): void {
  answerEl.innerHTML = formatExplainAnswer(r.response.answer);
  warningEl.textContent = r.response.warning ?? "";
  citationsEl.innerHTML = "";
  const kind = r.response.questionKind ? ` · ${r.response.questionKind}` : "";
  setStatus(`Lineup explain${kind} · ${r.response.elapsedMs}ms`);
}

function sendLineupExplain(question: string): void {
  setStatus("Explaining (local IQ)…");
  answerEl.textContent = "";
  citationsEl.innerHTML = "";
  warningEl.textContent = "";

  chrome.runtime.sendMessage(
    { type: "LINEUP_EXPLAIN", question, lineup: lastLineupResult ?? undefined } as SidecarMessage,
    (reply) => {
      if (chrome.runtime.lastError) {
        setStatus(chrome.runtime.lastError.message ?? "Error", true);
        return;
      }
      const r = reply as SidecarMessage;
      if (r.type === "ADVISE_ERROR") {
        setStatus(r.error, true);
        return;
      }
      if (r.type === "ADVISE_RESULT") renderExplainResult(r);
    }
  );
}

function explainRecommendation(): void {
  const question =
    questionInput.value.trim() ||
    "Explain the main differences between my lineup and the recommendation.";

  if (lastLineupResult) {
    sendLineupExplain(question);
    return;
  }

  setStatus("Building recommendation…");
  chrome.runtime.sendMessage({ type: "LINEUP_ANALYZE" } as SidecarMessage, (reply) => {
    if (chrome.runtime.lastError) {
      setStatus(chrome.runtime.lastError.message ?? "Error", true);
      return;
    }
    const r = reply as SidecarMessage;
    if (r.type === "ADVISE_ERROR") {
      setStatus(r.error, true);
      return;
    }
    if (r.type === "LINEUP_RESULT") {
      if (r.context) renderContext(r.context);
      lastLineupResult = r.response;
      renderLineupResult(r.response, r.context?.slots ?? currentContext?.slots);
      sendLineupExplain(question);
    }
  });
}

function ask(question: string): void {
  if (!question.trim()) return;
  if (isLineupPage()) {
    explainRecommendation();
    return;
  }

  setStatus("Thinking…");
  chartArea.innerHTML = "";
  answerEl.textContent = "";
  citationsEl.innerHTML = "";
  warningEl.textContent = "";

  chrome.runtime.sendMessage({ type: "ADVISE", question } as SidecarMessage, (reply) => {
    if (chrome.runtime.lastError) {
      setStatus(chrome.runtime.lastError.message ?? "Error", true);
      return;
    }
    const r = reply as SidecarMessage;
    if (r.type === "ADVISE_ERROR") {
      setStatus(r.error, true);
      return;
    }
    if (r.type === "ADVISE_RESULT") {
      answerEl.textContent = r.response.answer;
      warningEl.textContent = r.response.warning ?? "";
      citationsEl.innerHTML = r.response.citations
        .map(
          (c) =>
            `<li><strong>${escapeHtml(c.label)}</strong> <span class="tag">${escapeHtml(c.source)}</span>${c.snippet ? `<br><small>${escapeHtml(c.snippet)}</small>` : ""}</li>`
        )
        .join("");
      setStatus(`Done in ${r.response.elapsedMs}ms`);
    }
  });
}

function optimizeLineup(): void {
  setStatus("Reading lineup…");
  answerEl.textContent = "";
  citationsEl.innerHTML = "";
  warningEl.textContent = "";

  chrome.runtime.sendMessage({ type: "LINEUP_ANALYZE" } as SidecarMessage, (reply) => {
    if (chrome.runtime.lastError) {
      setStatus(chrome.runtime.lastError.message ?? "Error", true);
      return;
    }
    const r = reply as SidecarMessage;
    if (r.type === "ADVISE_ERROR") {
      setStatus(r.error, true);
      return;
    }
    if (r.type === "LINEUP_RESULT") {
      if (r.context) renderContext(r.context);
      lastLineupResult = r.response;
      renderLineupResult(r.response, r.context?.slots ?? currentContext?.slots);
    }
  });
}

askBtn.addEventListener("click", () => ask(questionInput.value));
optimizeBtn.addEventListener("click", () => optimizeLineup());

explainBtn.addEventListener("click", () => {
  const q =
    currentContext?.pageType === "roster"
      ? "Review this roster for salary balance, position coverage, and IR usage."
      : "Explain this screen and what I should check as a Classic Standard owner.";
  questionInput.value = q;
  ask(q);
});

questionInput.addEventListener("keydown", (e) => {
  if (e.key === "Enter" && !e.shiftKey) {
    e.preventDefault();
    ask(questionInput.value);
  }
});

renderLineupPrompts();

chrome.runtime.sendMessage({ type: "REFRESH_CONTEXT" } as SidecarMessage, (reply) => {
  if (chrome.runtime.lastError) {
    setStatus(chrome.runtime.lastError.message ?? "Could not read page", true);
    return;
  }
  const r = reply as SidecarMessage;
  if (r.type === "CONTEXT_UPDATE") {
    renderContext(r.context);
    setStatus("Ready");
  } else if (r.type === "ADVISE_ERROR") {
    setStatus(r.error, true);
  }
});
