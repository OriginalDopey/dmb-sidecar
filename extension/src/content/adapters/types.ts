import type { PageContext } from "../../shared/types.js";

export interface PageAdapter {
  readonly pageType: string;
  matches(url: URL): boolean;
  extract(document: Document, url: URL): PageContext;
}
