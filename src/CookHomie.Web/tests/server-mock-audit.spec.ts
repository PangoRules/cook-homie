import { describe, expect, it } from "vitest";
import { readFileSync, readdirSync, statSync } from "node:fs";
import { join } from "node:path";

const serverRoot = join(process.cwd(), "server");

const walk = (dir: string): string[] => readdirSync(dir).flatMap((entry) => {
  const path = join(dir, entry);
  return statSync(path).isDirectory() ? walk(path) : [path];
});

describe("server API mock audit", () => {
  it("requires obvious mock data in server routes to be marked TEMP_MOCK", () => {
    const offenders = walk(serverRoot)
      .filter(path => path.endsWith(".ts"))
      .filter((path) => {
        const source = readFileSync(path, "utf8");
        const looksMocked = [
          "Classic Pancakes",
          "Tomato Pasta",
          "Avocado Toast",
          "expiringCount: 3",
          "recipeMatchCount: 7",
          "shoppingCount: 4",
        ].some(marker => source.includes(marker));
        return looksMocked && !source.includes("TEMP_MOCK");
      });

    expect(offenders).toEqual([]);
  });
});
