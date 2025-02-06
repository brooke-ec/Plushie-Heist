import { env } from "$env/dynamic/private";
import { error } from "@sveltejs/kit";

export async function load({ request, setHeaders }) {
	const credentials: string[] = env.CREDENTIALS.split(",");

	function unauthorized(): never {
		setHeaders({ "WWW-Authenticate": 'Basic realm="Protected"' });
		error(401, "Unauthorized");
	}

	const authorization = request.headers.get("Authorization");
	if (!authorization || !authorization.startsWith("Basic ")) unauthorized();
	const token = authorization.replace("Basic ", "");

	const credential = Buffer.from(token, "base64").toString();
	if (!credentials.includes(credential)) unauthorized();
}
