import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import HomePage from './components/HomePage';
import RoomList from './components/RoomList';
// importez d'autres composants si nécessaire

function App() {
    return (
        <Router>
            <Switch>
                <Route exact path="/" component={HomePage} />
                <Route path="/rooms" component={RoomList} />
                {/* Ajoutez d'autres routes ici */}
            </Switch>
        </Router>
    );
}

export default App; 