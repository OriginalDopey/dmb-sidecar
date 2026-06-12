// src/sidepanel/sidepanel.ts
var pageLabel = document.getElementById("page-label");
var contextPreview = document.getElementById("context-preview");
var answerEl = document.getElementById("answer");
var citationsEl = document.getElementById("citations");
var warningEl = document.getElementById("warning");
var statusEl = document.getElementById("status");
var questionInput = document.getElementById("question");
var askBtn = document.getElementById("ask-btn");
var explainBtn = document.getElementById("explain-btn");
var currentContext = null;
function setStatus(text, isError = false) {
  statusEl.textContent = text;
  statusEl.className = isError ? "status error" : "status";
}
function renderContext(ctx) {
  currentContext = ctx;
  pageLabel.textContent = `${ctx.pageType} \u2014 ${new URL(ctx.url).pathname}`;
  const lines = [];
  if (ctx.lineupName) lines.push(`Lineup: ${ctx.lineupName}`);
  if (ctx.curTeam) lines.push(`Team: ${ctx.curTeam}`);
  if (ctx.slots?.length) {
    ctx.slots.slice(0, 9).forEach((s) => {
      lines.push(`${s.order}. ${s.playerName ?? "?"} ${s.position ? `(${s.position})` : ""}`);
    });
  }
  contextPreview.textContent = lines.length ? lines.join("\n") : "(no structured data \u2014 check DOM selectors)";
}
chrome.runtime.onMessage.addListener((msg) => {
  if (msg.type === "CONTEXT_UPDATE") renderContext(msg.context);
});
async function ask(question) {
  if (!question.trim()) return;
  setStatus("Thinking\u2026");
  answerEl.textContent = "";
  citationsEl.innerHTML = "";
  warningEl.textContent = "";
  chrome.runtime.sendMessage({ type: "ADVISE", question }, (reply) => {
    if (chrome.runtime.lastError) {
      setStatus(chrome.runtime.lastError.message ?? "Error", true);
      return;
    }
    const r = reply;
    if (r.type === "ADVISE_ERROR") {
      setStatus(r.error, true);
      return;
    }
    if (r.type === "ADVISE_RESULT") {
      answerEl.textContent = r.response.answer;
      warningEl.textContent = r.response.warning ?? "";
      citationsEl.innerHTML = r.response.citations.map(
        (c) => `<li><strong>${escapeHtml(c.label)}</strong> <span class="tag">${escapeHtml(c.source)}</span>${c.snippet ? `<br><small>${escapeHtml(c.snippet)}</small>` : ""}</li>`
      ).join("");
      setStatus(`Done in ${r.response.elapsedMs}ms`);
    }
  });
}
function escapeHtml(s) {
  return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
askBtn.addEventListener("click", () => ask(questionInput.value));
explainBtn.addEventListener("click", () => {
  const q = currentContext?.pageType === "lineup" ? "Review this lineup for platoon fit, batting order, and Classic Standard best practices. Cite rules where relevant." : currentContext?.pageType === "roster" ? "Review this roster for salary balance, position coverage, and IR usage." : "Explain this screen and what I should check as a Classic Standard owner.";
  questionInput.value = q;
  ask(q);
});
questionInput.addEventListener("keydown", (e) => {
  if (e.key === "Enter" && !e.shiftKey) {
    e.preventDefault();
    ask(questionInput.value);
  }
});
setStatus("Open an ImagineSports manage page\u2026");
//# sourceMappingURL=sidepanel.js.map
