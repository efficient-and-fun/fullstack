import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useEffect } from "react";
const ProtectedRoutes = () => {
  const token = localStorage.getItem("authToken");
  const navigate = useNavigate();
  const location = useLocation();
  useEffect(() => {
    const verifyToken = async () => {
      if (token) {
        try {
          const response = await fetch("api/user/validate", {
            method: "POST",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          });

          if (!response.ok) {
            localStorage.removeItem("authToken");
            navigate("/login", { replace: true });
          }
        } catch (error) {
          console.error("Token verification failed:", error);
          localStorage.removeItem("authToken");
          navigate("/login", { replace: true });
        }
      } else {
        navigate("/login", { replace: true });
      }
    };
    verifyToken();
  }, [token, navigate]);
  return token ? <Outlet /> : null;
};
export default ProtectedRoutes;
