import { useEffect, useState } from "react";
import './EditMeetUp.css';
import { Box, Typography } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import { MeetUpDetail } from "../../../models/MeetUpDetails";
import { createMeetUpApiCall, meetUpApiCall, updateMeetUpApiCall } from "../../../api/meetUpApi";

const EditMeetUp = () => {
    const { meetUpId } = useParams<{ meetUpId: string }>();
    const navigate = useNavigate();
    const isNew = !meetUpId;

    const [meetUp, setMeetUp] = useState<MeetUpDetail>({
        meetUpId: 0,
        meetUpName: '',
        dateTimeFrom: null,
        dateTimeTo: null,
        meetUpLocation: '',
        description: '',
        checkList: '',
        maxNumberOfParticipants: null
    });

    useEffect(() => {
        if (!isNew) {
            meetUpApiCall(setMeetUp, parseInt(meetUpId!));
        }
    }, [isNew, meetUpId]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setMeetUp((prev) => ({
            ...prev,
            [name.charAt(0).toLowerCase() + name.slice(1)]: value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log("Submitted data:", meetUp);
        if (!isNew) {
            updateMeetUpApiCall(
                meetUp,
                () => navigate(`/${meetUpId}`),
                (err) => alert(err)
            );
        } else {
            createMeetUpApiCall(
                meetUp,
                (newId) => navigate(`/${newId}`),
                (err) => alert(err)
            );
        }
    };

    const goBack = () => {
        navigate(isNew ? '/' : `/${meetUpId}`);
    };

    return (
        <Box className="edit-meetup-container">
            <Box className="edit-meetup-header">
                <Typography className="edit-meetup-title">
                    {isNew ? 'Create MeetUp' : 'Edit MeetUp'}
                </Typography>
            </Box>
            <Box className="edit-meetup-bottom-sheet">
                <form onSubmit={handleSubmit} className="edit-meetup-form">
                    <input
                        name="MeetUpName"
                        placeholder="Name"
                        value={meetUp.meetUpName}
                        onChange={handleChange}
                        required
                    />
                    <input
                        type="datetime-local"
                        name="DateTimeFrom"
                        value={meetUp.dateTimeFrom
                            ? new Date(meetUp.dateTimeFrom).toISOString().slice(0, 16)
                            : ''}
                        onChange={handleChange}
                        required
                    />
                    <input
                        type="datetime-local"
                        name="DateTimeTo"
                        value={meetUp.dateTimeTo
                            ? new Date(meetUp.dateTimeTo).toISOString().slice(0, 16)
                            : ''} onChange={handleChange}
                        required
                    />
                    <input
                        name="MeetUpLocation"
                        placeholder="Location"
                        value={meetUp.meetUpLocation}
                        onChange={handleChange}
                        required
                    />
                    <textarea
                        name="Description"
                        placeholder="Description"
                        value={meetUp.description}
                        onChange={handleChange}
                    />
                    <input
                        name="CheckList"
                        placeholder="Checklist"
                        value={meetUp.checkList}
                        onChange={handleChange}
                    />
                    <input
                        type="number"
                        name="maxNumberOfParticipants"
                        placeholder="Max. number of participants"
                        value={meetUp.maxNumberOfParticipants}
                        onChange={handleChange}
                    />
                    <div className="button-group">
                        <button type="submit" className="edit-meetup-submit">
                            {isNew ? 'Create' : 'Update'}
                        </button>
                        <button type="button" className="edit-meetup-cancel" onClick={goBack}>
                            Cancel
                        </button>
                    </div>
                </form>
            </Box>
        </Box>
    );
};

export default EditMeetUp;
