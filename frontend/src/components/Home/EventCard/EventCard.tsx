import './EventCard.css';
import { Typography, Card, CardContent } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import { MeetUp } from '../../../models/MeetUp';
import { useNavigate } from 'react-router-dom';

type EventCardProps = {
    meetUp: MeetUp;
};

const EventCard: React.FC<EventCardProps> = ({ meetUp: meetUp }) => {
    const hasTime = meetUp.dateTimeFrom && meetUp.dateTimeTo;

    const navigate = useNavigate();

    const navigateToDetailsPage = () => {
        navigate(`/${meetUp.meetUpId}`);
    };

    return (
        <Card onClick={navigateToDetailsPage} className="custom-card">
            <CardContent className="custom-card-content">
                <Typography variant="subtitle1" fontWeight="bold">
                    {meetUp.meetUpName}
                </Typography>

                {hasTime && (
                    <div className="custom-time">
                        <AccessTimeIcon fontSize="small" className="custom-time-icon" />
                        <Typography variant="body2" className="custom-time-text">
                            {meetUp.dateTimeFrom.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}{' '}
                            until{' '}
                            {meetUp.dateTimeTo.toLocaleTimeString([], {
                                hour: '2-digit',
                                minute: '2-digit',
                            })}
                        </Typography>
                    </div>
                )}
            </CardContent>
        </Card>
    );
};

export default EventCard;
