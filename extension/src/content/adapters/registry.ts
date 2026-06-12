import type { PageAdapter } from "./types.js";
import { lineupAdapter } from "./lineup.js";
import { rosterAdapter } from "./roster.js";

const adapters: PageAdapter[] = [lineupAdapter, rosterAdapter];

export function resolveAdapter(url: URL): PageAdapter | null {
  return adapters.find((a) => a.matches(url)) ?? null;
}

export function extractPageContext(document: Document, href: string) {
  const url = new URL(href);
  const adapter = resolveAdapter(url);
  if (!adapter) {
    return {
      pageType: "unknown",
      url: href,
      extra: { title: document.title },
    };
  }
  return adapter.extract(document, url);
}
