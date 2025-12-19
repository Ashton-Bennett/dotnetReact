import fs from "fs";
import path from "path";

const sourceDir = path.resolve("dist");
const targetDir = path.resolve("../Api/wwwroot");

const expectedSuffix = path.join("Api", "wwwroot");

if (!targetDir.endsWith(expectedSuffix)) {
  throw new Error(`Refusing to write outside Api/wwwroot: ${targetDir}`);
}

function copyRecursive(src, dest) {
  if (!fs.existsSync(dest)) {
    fs.mkdirSync(dest, { recursive: true });
  }

  for (const item of fs.readdirSync(src)) {
    const srcPath = path.join(src, item);
    const destPath = path.join(dest, item);

    // Remove only the existing item that will be replaced
    if (fs.existsSync(destPath)) {
      fs.rmSync(destPath, { recursive: true, force: true });
    }

    if (fs.statSync(srcPath).isDirectory()) {
      copyRecursive(srcPath, destPath);
    } else {
      fs.copyFileSync(srcPath, destPath);
    }
  }
}

copyRecursive(sourceDir, targetDir);
