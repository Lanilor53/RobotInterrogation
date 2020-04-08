import * as React from 'react';
import { IInterviewQuestion, InterviewQuestion } from './elements/InterviewQuestion';
import './ItemGroup.css';
import { InterviewPosition } from '../interviewReducer';
import { PositionHeader } from './elements/PositionHeader';

interface IProps {
    primary: IInterviewQuestion[],
    secondary: IInterviewQuestion[],
    waitingFor: string,
}

export const WaitingQuestionDisplay: React.FunctionComponent<IProps> = props => {
    const primary = props.primary.map((q, i) => <InterviewQuestion primary={true} question={q} key={i} />);
    const secondary = props.secondary.map((q, i) => <InterviewQuestion primary={false} question={q} key={i} />);

    return (
        <div>
            <PositionHeader position={InterviewPosition.Interviewer} />

            <p>Waiting for the suspect to select their {props.waitingFor}. Your questions are as follows:</p>
            <div className="itemGroup">
                {primary}
                {secondary}
            </div>
        </div>
    );
}