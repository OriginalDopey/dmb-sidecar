/**
 * @file Vitest configuration for the DMB Sidecar extension package.
 *
 * **Purpose:** Runs unit tests matching `src` test file glob with v8 coverage focused
 * on `lineup-format.ts` and enforces minimum coverage thresholds for CI.
 *
 * **Message flow:** N/A — build/test tooling only.
 *
 * **Dependencies:** `vitest/config` (`defineConfig`).
 */
import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    include: ["src/**/*.test.ts"],
    coverage: {
      provider: "v8",
      include: ["src/shared/lineup-format.ts"],
      reporter: ["text", "json-summary", "html"],
      reportsDirectory: "./coverage",
      thresholds: {
        lines: 90,
        functions: 90,
        branches: 85,
        statements: 90,
      },
    },
  },
});
