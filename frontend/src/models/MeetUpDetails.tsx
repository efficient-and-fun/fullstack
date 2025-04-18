export interface MeetUpDetail {
    MeetUpId: number;
    Name: string;
    Description?: string
    DateTimeFrom?: Date | null;
    DateTimeTo?: Date | null;
    Location: string;
}
