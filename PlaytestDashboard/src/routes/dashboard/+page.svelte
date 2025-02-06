<script lang="ts">
	import { faCalendar, faFileVideo, faHashtag } from "@fortawesome/free-solid-svg-icons";
	import PlaybackModal from "./PlaybackModal.svelte";
	import { browser } from "$app/environment";
	import { goto } from "$app/navigation";
	import { page } from "$app/state";
	import { Fa } from "svelte-fa";

	const { data } = $props();

	let file = $derived(page.url.hash.substring(1));
</script>

{#if file}
	<PlaybackModal {file} onclose={() => goto("#")} />
{/if}

<table>
	<thead>
		<tr>
			<td></td>
			<td><Fa icon={faHashtag} /> ID</td>
			<td><Fa icon={faCalendar} /> Date</td>
		</tr>
	</thead>
	<tbody>
		{#each data.uploads as recording}
			<tr>
				<td><a href="#{recording.file}"><Fa icon={faFileVideo} /></a></td>
				<td>{recording.id}</td>
				<td>{recording.date.toLocaleString(browser ? navigator.language : "en-GB")}</td>
			</tr>
		{/each}
	</tbody>
</table>

<style lang="scss">
	table {
		border: none;
		margin: 15px 10px;
	}

	thead td :global(svg) {
		padding-right: 5px;
	}

	tr {
		border: none;
	}

	tbody tr {
		border-top: solid 1px #ffffff65;
	}

	td {
		padding: 10px;
		border: none;
	}
</style>
