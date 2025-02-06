import { mkdir } from "node:fs/promises";
import { existsSync } from "node:fs";
import { UPLOADS_ROOT } from "$lib";
import fs from "node:fs/promises";
import path from "node:path";

export async function load() {
	if (!existsSync(UPLOADS_ROOT)) await mkdir(UPLOADS_ROOT);

	const files = await fs.readdir(UPLOADS_ROOT);

	const uploads = await Promise.all(
		files.map(async (file) => {
			const id = file.replace(/\.[^/.]+$/, "");
			const stat = await fs.stat(path.join(UPLOADS_ROOT, file));

			return { id, file, date: stat.ctime };
		}),
	);

	return { uploads };
}
