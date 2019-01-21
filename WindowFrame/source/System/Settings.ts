export class Settings {
    public static get Instance(): Settings { return window.Settings }

    public NotificationConditionValue: number = 49;
    public NotificationTime: number = 60;

    public IsLoSIncludeFirstFleet: boolean = true;
    public IsLoSIncludeSecondFleet: boolean = true;

    public LoSCalculator: string = "Cn1";

    public Ready(): Settings {
        // Placeholder
        return this;
    }
}