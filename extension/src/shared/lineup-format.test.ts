/**
 * @file Unit tests for `lineup-format.ts` display helpers.
 *
 * **Purpose:** Verifies baseball-style formatting rules (leading-zero strip,
 * em-dash sentinels, signed defensive values) used in Lineup Lab grid cells.
 *
 * **Message flow:** N/A — test-only; no extension IPC.
 *
 * **Dependencies:** `vitest`, `shared/lineup-format.js`.
 */
import { describe, expect, it } from "vitest";
import { fmtAvg, fmtDef, fmtRc600 } from "./lineup-format.js";

/** OBP/OPS-style rates: drop leading zero below 1, em dash for invalid values. */
describe("fmtAvg", () => {
  it("drops leading zero on sub-1 rates", () => {
    expect(fmtAvg(0.405, 3)).toBe(".405");
    expect(fmtAvg(0.912, 3)).toBe(".912");
  });

  it("keeps multi-digit values", () => {
    expect(fmtAvg(50.8, 1)).toBe("50.8");
  });

  it("returns em dash for missing or non-positive", () => {
    expect(fmtAvg(undefined, 3)).toBe("—");
    expect(fmtAvg(0, 3)).toBe("—");
    expect(fmtAvg(Number.NaN, 3)).toBe("—");
  });
});

/** RC/600 column: one decimal, same invalid sentinel as rate stats. */
describe("fmtRc600", () => {
  it("uses one decimal", () => {
    expect(fmtRc600(124.34)).toBe("124.3");
  });

  it("returns em dash for missing or non-positive", () => {
    expect(fmtRc600(undefined)).toBe("—");
    expect(fmtRc600(0)).toBe("—");
  });
});

/** Defensive runs: signed one decimal, zero bucket for near-neutral values. */
describe("fmtDef", () => {
  it("shows signed one decimal", () => {
    expect(fmtDef(6)).toBe("+6.0");
    expect(fmtDef(-0.5)).toBe("-0.5");
    expect(fmtDef(0.02)).toBe("0");
  });

  it("returns em dash when undefined", () => {
    expect(fmtDef(undefined)).toBe("—");
  });
});
