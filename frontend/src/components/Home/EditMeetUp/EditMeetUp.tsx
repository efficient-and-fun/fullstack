import { useEffect, useState } from "react";
import './EditMeetUp.css';
import { Box, Typography } from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import { MeetUpDetail } from "../../../models/MeetUpDetails";
import { createMeetUpApiCall, meetUpApiCall, updateMeetUpApiCall } from "../../../api/meetUpApi";

// Hilfsfunktion: Date oder ISO-String zu lokalem datetime-local String
function toLocalDateTimeString(date: Date | string | null) {
    if (!date) return '';
    const d = typeof date === 'string' ? new Date(date) : date;
    const tzOffset = d.getTimezoneOffset() * 60000;
    return new Date(d.getTime() - tzOffset).toISOString().slice(0, 16);
}

// Hilfsfunktion: datetime-local String zu Date
function localStringToDate(localString: string): Date | null {
    if (!localString) return null;
    return new Date(localString);
}

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

    // Lokale States für die Inputs, damit keine Zeitzonen-Konvertierung beim Tippen passiert
    const [dateTimeFromInput, setDateTimeFromInput] = useState('');
    const [dateTimeToInput, setDateTimeToInput] = useState('');

    useEffect(() => {
        if (!isNew) {
            meetUpApiCall((data: MeetUpDetail) => {
                setMeetUp({
                    ...data,
                    dateTimeFrom: data.dateTimeFrom ? new Date(data.dateTimeFrom) : null,
                    dateTimeTo: data.dateTimeTo ? new Date(data.dateTimeTo) : null,
                });
                setDateTimeFromInput(data.dateTimeFrom ? toLocalDateTimeString(data.dateTimeFrom) : '');
                setDateTimeToInput(data.dateTimeTo ? toLocalDateTimeString(data.dateTimeTo) : '');
            }, parseInt(meetUpId!));
        }
    }, [isNew, meetUpId]);

    // Standard-Handler für alle Felder außer datetime-local
    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value, type } = e.target;
        if (type === "number") {
            setMeetUp((prev) => ({
                ...prev,
                [name.charAt(0).toLowerCase() + name.slice(1)]: value === '' ? null : Number(value)
            }));
        } else {
            setMeetUp((prev) => ({
                ...prev,
                [name.charAt(0).toLowerCase() + name.slice(1)]: value
            }));
        }
    };

    // Speichert das Datum erst beim Verlassen des Feldes als Date im State
    const handleDateTimeBlur = (e: React.FocusEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setMeetUp((prev) => ({
            ...prev,
            [name.charAt(0).toLowerCase() + name.slice(1)]: value ? new Date(value) : null
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        // Beim Absenden: Date wird automatisch als ISO-String serialisiert
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
                        value={dateTimeFromInput}
                        onChange={e => setDateTimeFromInput(e.target.value)}
                        onBlur={handleDateTimeBlur}
                        required
                    />
                    <input
                        type="datetime-local"
                        name="DateTimeTo"
                        value={dateTimeToInput}
                        onChange={e => setDateTimeToInput(e.target.value)}
                        onBlur={handleDateTimeBlur}
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
                        value={meetUp.maxNumberOfParticipants ?? ''}
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