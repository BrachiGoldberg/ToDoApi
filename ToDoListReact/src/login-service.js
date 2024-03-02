import axios from "axios"
import myAxios from "./axiosConfiguration";

export default {

    login: async (userName, password) => {
        let result = await myAxios.post("/login", { Id: 0, UserName: userName, Password: password });
        localStorage.setItem("token_user", "Bearer " + result.data.token)
        return {}
    },

    logup: async (userName, password) => {
        let result = await myAxios.post("/logup", { Id: 0, UserName: userName, Password: password });
        localStorage.setItem("token_user", "Bearer " + result.data.token)
        return {}
    }

}