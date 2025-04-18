import './EventCard.css';
import { Typography, Chip, Stack, Card, CardContent } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import PlaceIcon from '@mui/icons-material/Place';
import { MeetUp } from '../../../models/MeetUp';
import { useNavigate } from 'react-router-dom';

type EventCardProps = {
    meetUp: MeetUp;
};

const EventCard: React.FC<EventCardProps> = ({ meetUp: meetUp }) => {
    const hasTime = meetUp.DateTimeFrom && meetUp.DateTimeTo;

    const navigate = useNavigate();

    const navigateToDetailsPage = () => {
        navigate(`/${meetUp.MeetUpId}`);
    };

    return (
        <Card onClick={navigateToDetailsPage} className="custom-card">
            <CardContent className="custom-card-content">
                <Typography variant="subtitle1" fontWeight="bold">
                    {meetUp.Name}
                </Typography>

                {hasTime && (
                    <div className="custom-time">
                        <AccessTimeIcon fontSize="small" className="custom-time-icon" />
                        <Typography variant="body2" className="custom-time-text">
                            {meetUp.DateTimeFrom.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}{' '}
                            until{' '}
                            {meetUp.DateTimeTo.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}
                        </Typography>
                    </div>
                )}

                <div className="custom-location">
                    <PlaceIcon fontSize="small" className="custom-location-icon" />
                    <Typography variant="body2" className="custom-location-text">
                        {meetUp.Location}
                    </Typography>
                </div>
            </CardContent>
        </Card>
    );
};

export default EventCard;
