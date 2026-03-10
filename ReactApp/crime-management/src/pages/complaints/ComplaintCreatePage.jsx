import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useConfiguration } from '../../context/ConfigurationContext';
import { useLoader } from '../../context/LoaderContext';
import AuthService from '../../services/AuthService';
import '../../styles/Global.css';

const ComplaintCreatePage = () => {
    const navigate = useNavigate();
    const { configuration } = useConfiguration();
    const { setLoading } = useLoader();

    // Form states
    const [complaintName, setComplaintName] = useState('');
    const [jurisdiction, setJurisdiction] = useState('');
    const [crimeType, setCrimeType] = useState('');
    const [victimName, setVictimName] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [crimeDescription, setCrimeDescription] = useState('');
    const [investigationDescription, setInvestigationDescription] = useState('');
    const [priorityLevel, setPriorityLevel] = useState('Low');
    const [reportDate, setReportDate] = useState('');
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');

    const [message, setMessage] = useState({ text: '', type: '' });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage({ text: '', type: '' });

        try {
            setLoading(true);

            const payload = {
                complaintName,
                jurisdiction,
                crimeType,
                victimName,
                phoneNumber,
                crimeDescription,
                investigationDescription,
                priorityLevel,
                reportDate,
                startDate,
                endDate: endDate || null // Handle optional end date properly depending on API expectations
            };

            const result = await AuthService.PostServiceCallToken('CrimeReport/RaiseCrimeReport', payload);

            if (result && result.success !== false) {
                setMessage({ text: 'Complaint raised successfully!', type: 'success' });

                // Redirect back to complaints list after a short delay
                setTimeout(() => {
                    navigate('/complaints');
                }, 1500);
            } else {
                setMessage({
                    text: result?.message || 'Failed to raise complaint. Please check your data.',
                    type: 'error'
                });
            }
        } catch (error) {
            console.error('Error raising complaint:', error);
            setMessage({
                text: error.message || 'An unexpected error occurred while raising the complaint.',
                type: 'error'
            });
        } finally {
            setLoading(false);
        }
    };

    // Quick inline styles for structured form
    const labelStyle = { display: 'block', marginBottom: '6px', fontWeight: 'bold', color: '#333' };
    const inputStyle = { width: '100%', padding: '10px', marginBottom: '16px', borderRadius: '4px', border: '1px solid #ccc', boxSizing: 'border-box' };

    return (
        <div style={{ padding: '20px', maxWidth: '900px', margin: '20px auto', backgroundColor: '#fff', borderRadius: '8px', boxShadow: '0 4px 10px rgba(0,0,0,0.1)' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px', borderBottom: '2px solid #ecf0f1', paddingBottom: '10px' }}>
                <h2 style={{ color: '#2c3e50', margin: 0 }}>Raise a New Complaint</h2>
                <button className="btn btn-link ps-0 text-decoration-none text-muted" onClick={() => navigate('/complaints')}>
                    <i className="bi bi-arrow-left me-1"></i> Back
                </button>
            </div>

            {message.text && (
                <div style={{
                    padding: '12px',
                    marginBottom: '20px',
                    borderRadius: '4px',
                    backgroundColor: message.type === 'success' ? '#d4edda' : '#f8d7da',
                    color: message.type === 'success' ? '#155724' : '#721c24',
                    border: `1px solid ${message.type === 'success' ? '#c3e6cb' : '#f5c6cb'}`
                }}>
                    {message.text}
                </div>
            )}

            <form onSubmit={handleSubmit}>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '30px' }}>

                    {/* Left Column */}
                    <div>
                        <label style={labelStyle}>Complaint Name</label>
                        <input
                            type="text"
                            style={inputStyle}
                            value={complaintName}
                            onChange={(e) => setComplaintName(e.target.value)}
                            required
                            placeholder="Enter complaint title"
                        />

                        <label style={labelStyle}>Jurisdiction</label>
                        <select
                            style={inputStyle}
                            value={jurisdiction}
                            onChange={(e) => setJurisdiction(e.target.value)}
                            required
                        >
                            <option value="">-- Select Jurisdiction --</option>
                            {configuration?.jurisdictionMaster?.map((item) => (
                                <option key={item.identifier} value={item.identifier}>
                                    {item.name}
                                </option>
                            ))}
                        </select>

                        <label style={labelStyle}>Crime Type</label>
                        <select
                            style={inputStyle}
                            value={crimeType}
                            onChange={(e) => setCrimeType(e.target.value)}
                            required
                        >
                            <option value="">-- Select Crime Type --</option>
                            {configuration?.crimeTypes?.map((item) => (
                                <option key={item.identifier} value={item.identifier}>
                                    {item.name}
                                </option>
                            ))}
                        </select>

                        <label style={labelStyle}>Victim Name</label>
                        <input
                            type="text"
                            style={inputStyle}
                            value={victimName}
                            onChange={(e) => setVictimName(e.target.value)}
                            required
                            placeholder="Enter victim's full name"
                        />

                        <label style={labelStyle}>Phone Number</label>
                        <input
                            type="tel"
                            style={inputStyle}
                            value={phoneNumber}
                            onChange={(e) => setPhoneNumber(e.target.value)}
                            required
                            placeholder="Enter 10-digit number"
                        />

                        <label style={labelStyle}>Priority Level</label>
                        <select
                            style={inputStyle}
                            value={priorityLevel}
                            onChange={(e) => setPriorityLevel(e.target.value)}
                            required
                        >
                            <option value="Low">Low</option>
                            <option value="Medium">Medium</option>
                            <option value="High">High</option>
                        </select>
                    </div>

                    {/* Right Column */}
                    <div>
                        <label style={labelStyle}>Report Date</label>
                        <input
                            type="date"
                            style={inputStyle}
                            value={reportDate}
                            onChange={(e) => setReportDate(e.target.value)}
                            required
                        />

                        <label style={labelStyle}>Start Date</label>
                        <input
                            type="date"
                            style={inputStyle}
                            value={startDate}
                            onChange={(e) => setStartDate(e.target.value)}
                            required
                        />

                        <label style={labelStyle}>End Date</label>
                        <input
                            type="date"
                            style={inputStyle}
                            value={endDate}
                            onChange={(e) => setEndDate(e.target.value)}
                        />

                        <label style={labelStyle}>Crime Description</label>
                        <textarea
                            style={{ ...inputStyle, minHeight: '100px', resize: 'vertical' }}
                            value={crimeDescription}
                            onChange={(e) => setCrimeDescription(e.target.value)}
                            required
                            placeholder="Detailed description of the crime..."
                        ></textarea>

                        <label style={labelStyle}>Investigation Description</label>
                        <textarea
                            style={{ ...inputStyle, minHeight: '100px', resize: 'vertical' }}
                            value={investigationDescription}
                            onChange={(e) => setInvestigationDescription(e.target.value)}
                            placeholder="Initial investigation observations..."
                        ></textarea>
                    </div>
                </div>

                <div style={{ marginTop: '20px', display: 'flex', justifyContent: 'flex-end', gap: '15px' }}>
                    <button
                        type="button"
                        onClick={() => navigate('/complaints')}
                        style={{ padding: '12px 24px', fontSize: '1rem', cursor: 'pointer', backgroundColor: '#e74c3c', color: '#fff', border: 'none', borderRadius: '4px', fontWeight: 'bold' }}
                    >
                        Cancel
                    </button>
                    <button
                        type="submit"
                        style={{ padding: '12px 24px', fontSize: '1rem', cursor: 'pointer', backgroundColor: '#2ecc71', color: '#fff', border: 'none', borderRadius: '4px', fontWeight: 'bold' }}
                    >
                        Submit Complaint
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ComplaintCreatePage;
