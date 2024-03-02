import React, { useEffect, useState } from 'react';
import Login from './Login.js';
import Task from './Task.js';
import {  Outlet, Route, Routes, useNavigate } from 'react-router-dom';
import Logup from './Logup.js';

const App = () => {

  const navigate = useNavigate();

  return <>
    <Routes>

      <Route path='/' element={useEffect(() => {navigate('/login')  },[])} />
      <Route path='/login' element={<Login ></Login>} ></Route>
      <Route path='/logup' element={<Logup ></Logup>} ></Route>
      <Route path='/tasks' element={<Task></Task>} ></Route>
    </Routes>

    <Outlet />

  </>
}

export default App;