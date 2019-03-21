/** Master information interface for Account items */
export interface kcsapi_mst_useitem {
	/** Internal item id */
	api_id: number;

	/** Type of item
	 * 
	 * - 0 : Not consumable
	 * - 1 : Instant repair bucket
	 * - 2 : Instant construction material
	 * - 3 : Development material
	 * - 4 : Consumable in Inventory
	 * - 5 : ~~dummy~~
	 * - 6 : Resources (Fuel, Ammo, Steel, Bauxite, Furniture coin)
	 * - 7 : ~~dummy~~
	 */
	api_usetype: number;

	/** Category of type
	 * 
	 * - 0 : Not categorized
	 * - 1 : Instant repair bucket
	 * - 2 : Instant construction material
	 * - 3 : Development material
	 * - 4 : Equipment improvement material
	 * - 5 : ~~undefined~~
	 * - 6 : Furniture coin **box**
	 * - 7~15 : ~~undefined~~
	 * - 16 : Fuel
	 * - 17 : Ammo
	 * - 18 : Steel
	 * - 19 : Bauxite
	 * - 20 : ~~undefined~~
	 * - 21 : Furniture coin
	 */
	api_category: number;

	/** Name */
	api_name: string;

	/** Description of item
	 * `[Description, Extra Description]`
	 * 
	 * Example of Extra Description: Coin amount of Furniture coin box.
	 */
	api_description: [string, string];

	/** Item price when buy at shop, unused */
	api_price: number;
}