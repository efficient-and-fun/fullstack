export interface MeetUpDetail {
    meetUpId: number;
    meetUpName: string;
    dateTimeFrom?: Date | null;
    dateTimeTo?: Date | null;
    meetUpLocation: string;
    description?: string
}
