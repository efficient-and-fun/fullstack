import styles from "./Form.module.css";
import { useNavigate } from "react-router-dom";

const LogOut = () => {

    const navigate = useNavigate();
    
    const handleLogOut = async (e) => {
        localStorage.removeItem("authToken");
        localStorage.removeItem("userId");
        navigate("/login", { replace: true });
    }

    return (
      <button className={styles.btn} onClick={handleLogOut}>
        Log Out
      </button>
    );
}

export default LogOut;