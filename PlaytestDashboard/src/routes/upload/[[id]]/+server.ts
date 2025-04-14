import { createWriteStream, existsSync } from "node:fs";
import { unlink, mkdir } from "node:fs/promises";
import { pipeline } from "node:stream/promises";
import { Readable } from "node:stream";
import { UPLOADS_ROOT } from "$lib";
import path from "node:path";

export async function PUT({ request, params }) {
	let id = params.id;
	if (!id) id = crypto.randomUUID().replaceAll("-", "");
	else if (!id.match(/^[0-9a-z]+$/i))
		return new Response("Id must be alphanumeric", { status: 400 });

	const filePath = path.normalize(path.join(UPLOADS_ROOT, `${id}.mp4`));
	if (!existsSync(UPLOADS_ROOT)) await mkdir(UPLOADS_ROOT);

	if (request.body === null) return new Response("Request has no body", { status: 400 });
	const wstream = createWriteStream(filePath);

	// @ts-ignore
	const rstream = Readable.fromWeb(request.body);

	try {
		await pipeline(rstream, wstream);
	} catch (e) {
		console.error(e);
		unlink(filePath);
		return new Response("Write to disk failed", { status: 500 });
	}

	return new Response(id, { status: 201 });
}
