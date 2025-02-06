<script lang="ts">
	import { faXmark } from "@fortawesome/free-solid-svg-icons";
	import Fa from "svelte-fa";

	let { file, onclose }: { file: string; onclose?: () => {} } = $props();

	let clientWidth: number = $state(0);
	let video: HTMLVideoElement;
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="background"></div>
<div class="container">
	<div class="title" style="width: {clientWidth}px;">
		<h1>{file}</h1>
		<button onclick={onclose}><Fa icon={faXmark} /></button>
	</div>
	<!-- svelte-ignore a11y_media_has_caption -->
	<video bind:this={video} bind:clientWidth controls src="uploads/{file}"></video>
</div>

<style lang="scss">
	.background {
		background-color: #00000041;
		backdrop-filter: blur(10px);
		position: fixed;

		left: 0px;
		top: 0px;

		height: 100vh;
		width: 100vw;
	}

	.container {
		justify-content: center;
		flex-direction: column;
		align-items: center;
		position: fixed;
		display: flex;
		height: 100vh;
		width: 100vw;
		gap: 30px;
	}

	.title {
		justify-content: end;
		align-items: center;
		display: flex;

		h1 {
			margin-right: auto;
			text-align: start;
		}

		button {
			font-size: 40px;
		}
	}

	video {
		border-radius: 10px;
		max-height: 55vh;
		max-width: 55vw;
	}
</style>
