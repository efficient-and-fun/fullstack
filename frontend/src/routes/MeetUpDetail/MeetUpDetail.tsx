import './MeetUpDetail.css';
import { useNavigate, useParams } from 'react-router-dom';
import { Box, Typography, IconButton, Stack } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import PlaceIcon from '@mui/icons-material/Place';
import CloseIcon from '@mui/icons-material/Close';
import eventsRaw from '../../components/Home/DailyView/testEvents.json';
import { MeetUpDetail } from '../../models/MeetUpDetails';

const MeetUpDetailPage: React.FC = () => {
    const { meetUpId } = useParams<{ meetUpId: string }>();
    const navigate = useNavigate();

    const events = eventsRaw.map(event => ({
        ...event,
        DateTimeFrom: event.DateTimeFrom ? new Date(event.DateTimeFrom) : null,
        DateTimeTo: event.DateTimeTo ? new Date(event.DateTimeTo) : null,
    }));

    const meetUp = events.find(event => event.MeetUpId === parseInt(meetUpId, 10));
    const hasTime = meetUp.DateTimeFrom && meetUp.DateTimeTo;
    const hasDescription = meetUp.Description;

    return (
        <Box className="meetup-detail-container">
            {/* Header */}
            <Box className="meetup-header">
                <IconButton onClick={() => navigate('/')} className="meetup-close-button">
                    <CloseIcon />
                    <Typography variant="body2">CLOSE</Typography>
                </IconButton>
            </Box>

            {/* Time & Location */}
            <Box className="meetup-info-section">
                {hasTime && (
                    <Box className="meetup-info-group">

                        <AccessTimeIcon fontSize="small" sx={{ color: 'limegreen' }} />
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
                    </Box>)}
                <Box className="meetup-info-group">
                    <PlaceIcon fontSize="small" sx={{ color: 'limegreen' }} />
                    <Typography color="limegreen">{meetUp.Location}</Typography>
                </Box>
            </Box>

            {/* Title & Description */}
            <Box className="meetup-title-section">
                <Typography variant="h4" fontWeight="bold">
                    {meetUp.Name}
                </Typography>
                {hasDescription && (<Typography variant="body1" className="meetup-description">
                    {meetUp.Description}
                </Typography>)}
            </Box>
        </Box>
    );
};

export default MeetUpDetailPage;
