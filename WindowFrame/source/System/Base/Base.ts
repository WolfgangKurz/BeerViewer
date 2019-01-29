/** Calls `Handler` even if it is array. */
export function fns<T extends Function | any>(Handler: T | T[] | undefined | null, ...Params: any) {
	if (Handler) {
		if ((<T[]>Handler).length)
			(<T[]>Handler).forEach(x => typeof x === "function" && x(...Params));
		else if (typeof Handler === "function")
			(<T>Handler)(...Params);
	}
}

/** Wrap object for same context working */
export function wrap<T extends object>(Target: T, Wrapper: (Target: T) => void) {
	Wrapper.call(Target, Target);
}