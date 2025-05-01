import { Outlet, useNavigate } from "react-router-dom";
import { useEffect } from "react";
const ProtectedRoutes = () => {
  const token = localStorage.getItem("authToken");
  const navigate = useNavigate();
  useEffect(() => {
    const verifyToken = async () => {
      if (token) {
        console.log("send validate");
        try {
          const response = await fetch("api/validate", {
            method: "GET",
            headers: {
              Authorization: "Bearer ${token}",
            },
          });

          if (!response.ok) {
            console.log("noValid Token")
            localStorage.removeItem("authToken");
            navigate("/login", { replace: true });
          }
        } catch (error) {
          console.error("Token verification failed:", error);
          localStorage.removeItem("authToken");
          navigate("/login", { replace: true });
        }
      } else {
        console.log("redirected");
        navigate("/login", { replace: true });
      }
    };
    verifyToken();
  }, [token, navigate]);
  return token ? <Outlet /> : null;
};
export default ProtectedRoutes;
