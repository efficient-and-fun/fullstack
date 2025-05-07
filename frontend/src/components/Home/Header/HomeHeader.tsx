import React from "react";
import './HomeHeader.css';
import { useNavigate } from "react-router-dom";

const HomeHeader = () => {

  const navigate = useNavigate();

  return (
    <div className="headercontainer">
      <button
        className="btn"
        onClick={() => navigate('/new')}
      >
        +
      </button>
    </div>
  );
};

export default HomeHeader;
