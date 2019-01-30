/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />

declare interface Symbol {
	readonly description: string | undefined;
}
declare module '*.html' {
	const content: string;
	export default content;
  }
  