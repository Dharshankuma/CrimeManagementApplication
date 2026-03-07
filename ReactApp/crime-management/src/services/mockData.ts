
export const COMPLAINTS = [
  { id: 'C1001', name: 'Grand Theft Auto - Downtown', jurisdiction: 'Zone A', type: 'Robbery', status: 'Investigation', date: '2023-10-25' },
  { id: 'C1002', name: 'Cyber Fraud Reporting', jurisdiction: 'Cyber Cell', type: 'Cyber Crime', status: 'New', date: '2023-10-26' },
  { id: 'C1003', name: 'Assault at Central Park', jurisdiction: 'Zone B', type: 'Assault', status: 'Resolved', date: '2023-10-24' },
  { id: 'C1004', name: 'Domestic Dispute', jurisdiction: 'Zone A', type: 'Domestic', status: 'Closed', date: '2023-10-22' },
  { id: 'C1005', name: 'Suspicious Activity', jurisdiction: 'Zone C', type: 'Intelligence', status: 'Open', date: '2023-10-27' },
];

export const STATUS_WORKFLOW: Record<string, string[]> = {
  'New': ['Open', 'Closed - No Action'],
  'Open': ['Investigation', 'Closed - No Action'],
  'Investigation': ['Resolved', 'Closed - No Action'],
  'Resolved': ['Closed'],
  'Closed': [],
  'Closed - No Action': []
};

export const MOCK_EVIDENCE = [
  { id: 'E1', name: 'CCTV_Footage_Oct25.mp4', size: '45MB', type: 'Video', date: '2023-10-25' },
  { id: 'E2', name: 'Weapon_Photo_001.jpg', size: '2MB', type: 'Image', date: '2023-10-25' },
  { id: 'E3', name: 'Witness_Statement_A.pdf', size: '150KB', type: 'Document', date: '2023-10-26' },
];

export const MOCK_COMMENTS = [
  { id: 'CM1', user: 'Officer Smith', text: 'Secured the perimeter and collected fingerprints.', date: '2023-10-25 14:30' },
  { id: 'CM2', user: 'Forensic Lab', text: 'DNA samples have been sent for processing.', date: '2023-10-26 09:15' },
];

export const MOCK_TIMELINE = [
  { from: 'New', to: 'Open', changedBy: 'Admin', date: '2023-10-25 10:00' },
  { from: 'Open', to: 'Investigation', changedBy: 'Officer Smith', date: '2023-10-25 12:00' },
];
