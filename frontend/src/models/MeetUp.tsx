export interface MeetUp {
    EventId: number;
    Name: string;
    Description?: string | null;
    DateTimeFrom?: Date | null;
    DateTimeTo?: Date | null;
    Location: string;
    Tags: string[]
  }
  