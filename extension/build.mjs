import * as esbuild from "esbuild";
import { mkdirSync } from "fs";

mkdirSync("dist", { recursive: true });

const watch = process.argv.includes("--watch");

const ctx = await esbuild.context({
  entryPoints: {
    background: "src/background/background.ts",
    content: "src/content/content.ts",
    sidepanel: "src/sidepanel/sidepanel.ts",
  },
  bundle: true,
  outdir: "dist",
  format: "esm",
  target: "chrome114",
  sourcemap: true,
});

if (watch) {
  await ctx.watch();
  console.log("Watching extension...");
} else {
  await ctx.rebuild();
  await ctx.dispose();
  console.log("Built extension → dist/");
}
