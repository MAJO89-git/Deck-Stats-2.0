import { useNavigate } from 'react-router-dom'

function Dashboard() {
    const navigate = useNavigate()
    const handleLogout = () => {
        navigate('/')
    }


    return (
        <div>
            <h1>Welcome to the Dashboard</h1>
            <button onClick={handleLogout}>Logout</button>
        </div>
        

    )
}

export default Dashboard