import type { PageContext, SidecarMessage } from "../shared/types.js";

const pageLabel = document.getElementById("page-label")!;
const contextPreview = document.getElementById("context-preview")!;
const answerEl = document.getElementById("answer")!;
const citationsEl = document.getElementById("citations")!;
const warningEl = document.getElementById("warning")!;
const statusEl = document.getElementById("status")!;
const questionInput = document.getElementById("question") as HTMLTextAreaElement;
const askBtn = document.getElementById("ask-btn")!;
const explainBtn = document.getElementById("explain-btn")!;

let currentContext: PageContext | null = null;

function setStatus(text: string, isError = false): void {
  statusEl.textContent = text;
  statusEl.className = isError ? "status error" : "status";
}

function renderContext(ctx: PageContext): void {
  currentContext = ctx;
  pageLabel.textContent = `${ctx.pageType} — ${new URL(ctx.url).pathname}`;
  const lines: string[] = [];
  if (ctx.lineupName) lines.push(`Lineup: ${ctx.lineupName}`);
  if (ctx.curTeam) lines.push(`Team: ${ctx.curTeam}`);
  if (ctx.slots?.length) {
    ctx.slots.slice(0, 9).forEach((s) => {
      lines.push(`${s.order}. ${s.playerName ?? "?"} ${s.position ? `(${s.position})` : ""}`);
    });
  }
  contextPreview.textContent = lines.length ? lines.join("\n") : "(no structured data — check DOM selectors)";
}

chrome.runtime.onMessage.addListener((msg: SidecarMessage) => {
  if (msg.type === "CONTEXT_UPDATE") renderContext(msg.context);
});

async function ask(question: string): Promise<void> {
  if (!question.trim()) return;
  setStatus("Thinking…");
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

function escapeHtml(s: string): string {
  return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

askBtn.addEventListener("click", () => ask(questionInput.value));
explainBtn.addEventListener("click", () => {
  const q =
    currentContext?.pageType === "lineup"
      ? "Review this lineup for platoon fit, batting order, and Classic Standard best practices. Cite rules where relevant."
      : currentContext?.pageType === "roster"
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

setStatus("Open an ImagineSports manage page…");
