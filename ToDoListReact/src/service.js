import axios from 'axios';
import myAxios, { headerAxios } from './axiosConfiguration';
import { Navigate, useNavigate } from 'react-router-dom';


export default {
  getTasks: async () => {
    headerAxios();
    const result = await myAxios.get(`/tasks`)
    if (result == undefined || result.data == undefined)
      return [];
    else
      return result.data;
  },

  addTask: async (name) => {
    headerAxios();
    const result = await myAxios.post(`/tasks`, { Id: 0, Name: name, IsCompelte: false })
    return {};
  },

  setCompleted: async (id, isComplete) => {
    headerAxios();
    const result = await myAxios.put(`/tasks/${id}`, { Id: id, Name: "", IsCompelte: isComplete });
    return {};
  },

  deleteTask: async (id) => {
    headerAxios();
    const result = myAxios.delete(`/tasks/${id}`);
    return {};
  }

  
};
