import Vuex, { GetterTree, ActionTree, MutationTree, Store } from "vuex";

interface MutationPrototype {
	__mutations__: any[];
}
interface MutationTarget {
	prototype: MutationPrototype;
}

/**
 * `@MutationMethod()` decorator
 */
export function MutationMethod() {
	return function(target: any, propertyKey: string, descriptor: PropertyDescriptor) {
		const mtarget = target[propertyKey] as MutationTarget;
		const proto = Object.getPrototypeOf(mtarget);
		if (!proto.__mutations__)
			proto.__mutations__ = [];
		proto.__mutations__.push(mtarget);
	};
}

/**
 * Convert class instance to Vuex store
 * @param target Class to convert to Vuex store
 * @returns Converted Vuex store
 */
export default function ConvertStore(target: any): Store<any> {
	const ret = {
		state: {},
		getters: {} as GetterTree<any, any>,
		actions: {} as ActionTree<any, any>,
		mutations: {} as MutationTree<any>,
		strict: process.env.NODE_ENV !== "production"
	};

	const states = Object.getOwnPropertyDescriptors(target);
	Object.keys(states)
		.filter((x) => !x.startsWith("$_"))
		.forEach((x) => {
			Object.defineProperty(ret.state, x, {
				get: () => target[x],
				set: (v) => target[x] = v,
				configurable: true,
				enumerable: true,
				// writable: desc.writable,
			});
		});

	const funcs = Object.getOwnPropertyDescriptors(Object.getPrototypeOf(target));
	Object.keys(funcs)
		.filter((x) => x !== "constructor" && !x.startsWith("$_"))
		.forEach((x) => {
			const desc = funcs[x];
			if (desc.get && desc.set) return; // unknown!

			if (desc.get) // getter
				ret.getters[x] = () => target[x];
			else {// action or mutation
				const mtarget = target[x] as MutationTarget;
				const proto = Object.getPrototypeOf(mtarget);
				if (proto.__mutations__ && proto.__mutations__.includes(mtarget))
					ret.mutations[x] = (state, payload) => target[x].call(state, payload); // mutation
				else
					ret.actions[x] = (context, payload) => target[x].call(context, payload); // action
			}
		});

	return new Vuex.Store(ret);
}
