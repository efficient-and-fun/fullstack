import './EventCard.css';
import { Typography, Chip, Stack, Card, CardContent } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import PlaceIcon from '@mui/icons-material/Place';
import { MeetUp } from '../../../models/MeetUp';

type EventCardProps = {
    event: MeetUp;
};

const EventCard: React.FC<EventCardProps> = ({ event }) => {
    const hasTime = event.DateTimeFrom && event.DateTimeTo;
    const hasDescription = event.Description && event.Description.trim() !== '';

    return (
        <Card className="custom-card">
            <CardContent className="custom-card-content">
                <Typography variant="subtitle1" fontWeight="bold">
                    {event.Name}
                </Typography>

                {hasDescription && (
                    <Typography variant="body2" color="grey.400" mb={2}>
                        {event.Description}
                    </Typography>
                )}

                {hasTime && (
                    <div className="custom-time">
                        <AccessTimeIcon fontSize="small" className="custom-time-icon" />
                        <Typography variant="body2" className="custom-time-text">
                            {event.DateTimeFrom.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}{' '}
                            until{' '}
                            {event.DateTimeTo.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}
                        </Typography>
                    </div>
                )}

                <div className="custom-location">
                    <PlaceIcon fontSize="small" className="custom-location-icon" />
                    <Typography variant="body2" className="custom-location-text">
                        {event.Location}
                    </Typography>
                </div>

                <div>
                    {event.Tags.map((tag, index) => (
                        <Chip key={index} label={tag} className="custom-chip" />
                    ))}
                </div>
            </CardContent>
        </Card>
    );
};

export default EventCard;
