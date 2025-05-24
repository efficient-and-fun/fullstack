import { Outlet, useNavigate, useLocation } from "react-router-dom";
import { useEffect } from "react";
import { validateApiCall } from "../api/meetUpApi";
const ProtectedRoutes = () => {
  const token = localStorage.getItem("authToken");
  const navigate = useNavigate();
  const location = useLocation();
  const endpoint = "/validate";
  useEffect(() => {
    const verifyToken = async () => {
      if (token) {
        const data = await validateApiCall(endpoint);

        if (!data.ok) {
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
