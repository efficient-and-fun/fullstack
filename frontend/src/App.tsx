import "./App.css";

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './routes/Home';
import { Box } from '@mui/material';
import EventPage from './routes/Event';
import FriendsPage from './routes/Friends';
import NotificationPage from './routes/Notification';
import SettingPage from './routes/Setting';
import MeetUpDetailPage from "./routes/MeetUpDetail/MeetUpDetail";
import Nav from './components/Navbar/Nav';
import EditMeetUp from "./components/Home/EditMeetUp/EditMeetUp";


function App() {

  return (
    <>
      <Router>
        <Box>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/event" element={<EventPage />} />
            <Route path="/friends" element={<FriendsPage />} />
            <Route path="/notification" element={<NotificationPage />} />
            <Route path="/setting" element={<SettingPage />} />
            <Route path="/:meetUpId" element={<MeetUpDetailPage />} />
            <Route path="/new" element={<EditMeetUp />} />
            <Route path="/:meetUpId/edit" element={<EditMeetUp />} />
          </Routes>
          <Nav />
        </Box>
      </Router>
    </>
  );
}

export default App;
