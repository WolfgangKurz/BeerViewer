class Homeport {
    public static readonly Instance: Homeport = new Homeport;

    public Admiral: Admiral | null = null;
    public Materials: Materials | null = null;
    public Ships: Ship[] = [];

    constructor() {

    }
}