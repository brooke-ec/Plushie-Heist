import { pipeline } from "node:stream/promises";
import { createWriteStream } from "node:fs";
import { unlink } from "node:fs/promises";
import { Readable } from "node:stream";
import { error } from "@sveltejs/kit";
import { UPLOADS_ROOT } from "$lib";
import path from "node:path";

export async function PUT({ request }) {
	const filename = `${crypto.randomUUID().replaceAll("-", "")}.mp4`;
	const filePath = path.normalize(path.join(UPLOADS_ROOT, filename));

	if (request.body === null) error(400, "No body provided");

	const wstream = createWriteStream(filePath);

	// @ts-ignore
	const rstream = Readable.fromWeb(request.body);

	try {
		await pipeline(rstream, wstream);
	} catch (_) {
		unlink(filePath);
		error(500, "Failed to upload");
	}

	return new Response("File uploaded!", { status: 201 });
}
