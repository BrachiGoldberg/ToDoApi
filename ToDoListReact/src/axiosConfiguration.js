import axios from "axios";


let myAxios = axios.create({
    baseURL: "https://localhost:7078",
})

myAxios.interceptors.response.use(
    function(response) {
      return response;
    },
   async function(error) {
    console.log(error);
      // const res = await axios.post("https://localhost:7078/error", {error: error.message});
      if (error.response.status === 401) {
        return (window.location.href = "/login");
      }
    }
  );

export const headerAxios = () => {
    myAxios.defaults.headers = { "Authorization": localStorage.getItem('token_user') }
}

export default myAxios