import { useState } from "react";
import './NewMeetUp.css'
import { Box, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

const NewMeetUp = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        MeetUpName: '',
        DateTimeFrom: '',
        DateTimeTo: '',
        Checklist: '',
        MeetUpLocation: '',
        Description: '',
        AmountOfParticipants: '',
    });

    const handleChange = (e) => {
        console.log("handle change")
    };

    const handleSubmit = async (e) => {
        console.log("handle submit")
        navigate('/')
    };

    return (
        <Box className="new-meetup-container">
            <Box className="new-meetup-header">
                <Typography className="new-meetup-title">Create MeetUp</Typography>
            </Box>
            <Box className="new-meetup-bottom-sheet">
                <form onSubmit={handleSubmit} className="new-meetup-form">
                    <input name="MeetUpName" placeholder="Name" onChange={handleChange} required />
                    <input type="datetime-local" name="DateTimeFrom" onChange={handleChange} required />
                    <input type="datetime-local" name="DateTimeTo" onChange={handleChange} required />
                    <input name="Checklist" placeholder="Checklist (comma-separated)" onChange={handleChange} />
                    <input name="MeetUpLocation" placeholder="Location" onChange={handleChange} required />
                    <textarea name="Description" placeholder="Description" onChange={handleChange} />
                    <input type="number" name="AmountOfParticipants" placeholder="Participants" onChange={handleChange} required />
                    <button type="submit" className="new-meetup-submit">Create</button>
                    <button type="button" className="new-meetup-cancel" onClick={() => navigate('/')}>Cancel</button>
                </form>
            </Box>
        </Box>

    );
};

export default NewMeetUp;
