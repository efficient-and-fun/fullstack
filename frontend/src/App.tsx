import "./App.css";

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import HomePage from './routes/Home';
import EventPage from './routes/Event';
import FriendsPage from './routes/Friends';
import NotificationPage from './routes/Notification';
import SettingPage from './routes/Setting';


function App() {

  return (
    <>
    <Router>  
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/event" element={<EventPage />} />
          <Route path="/friends" element={<FriendsPage />} />
          <Route path="/notification" element={<NotificationPage />} />
          <Route path="/setting" element={<SettingPage />} />
        </Routes>
    </Router>
    </>
  );
}

export default App;
